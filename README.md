# Observer Scheduler

A robust multi-tenant job scheduling service built with ASP.NET Core 8.0 that allows users to create, manage, and execute scheduled HTTP GET requests based on cron expressions. The service features secure API key authentication, URL encryption, and comprehensive job logging.

## Features

- **Multi-Tenancy Support**: Each user has isolated job collections in MongoDB
- **Cron-Based Scheduling**: Schedule jobs using standard cron expressions with timezone support
- **HTTP Job Execution**: Execute HTTP GET requests to specified URLs
- **Retry Mechanism**: Configurable retry count and delay for failed jobs
- **URL Encryption**: Secure URL storage using AES-256-GCM encryption
- **Job Logging**: Detailed execution logs with status tracking (Success/Failed)
- **API Key Authentication**: Secure access control with encrypted API keys
- **Role-Based Authorization**: Admin and Stakeholder roles for user management
- **Background Job Processing**: Automated job execution via background service
- **PostgreSQL + MongoDB**: Hybrid database architecture for user data and job storage

## Architecture

### Technology Stack

- **Framework**: ASP.NET Core 8.0 (Minimal APIs)
- **Databases**:
  - PostgreSQL (User management with Entity Framework Core)
  - MongoDB (Job storage and logs with per-tenant collections)
- **Job Scheduling**: Quartz.NET (Cron expression parsing)
- **Object Mapping**: AutoMapper
- **Encryption**: AES-256-GCM for URL security
- **API Documentation**: Swagger/OpenAPI

### Project Structure

```
ObserverScheduler/
├── Abstractions/          # Service and repository interfaces
├── Api/                   # API endpoints and middlewares
│   ├── Endpoints.cs       # Main endpoint configuration
│   ├── JobEndpoints.cs    # Job-related endpoints
│   ├── UserEndpoints.cs   # User management endpoints
│   └── Middlewares/       # Custom middleware components
├── Common/                # Constants and utilities
├── Data/                  # Entity Framework context and mappings
├── Entities/              # Database entities
├── Extensions/            # Service registration extensions
├── Helper/                # Encryption and utility helpers
├── Migrations/            # EF Core migrations
├── Models/                # DTOs and view models
├── Repositories/          # Data access layer
└── Service/               # Business logic implementation
```

## Prerequisites

- **.NET 8.0 SDK** or later
- **Docker** and **Docker Compose** (for containerized deployment)
- **PostgreSQL** (version 12 or later)
- **MongoDB** (version 4.4 or later)

## Installation & Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ObserverScheduler
```

### 2. Create Encryption Key File

**Important**: Create a `cipher.txt` file in the project root containing a 32-byte AES-256 encryption key.

```bash
# Generate a random 32-byte key (example using OpenSSL)
openssl rand -base64 32 | head -c 32 > cipher.txt
```

### 3. Configure Database Connections

Update `appsettings.json` with your database connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=yourpassword;Host=localhost;Port=5432;Database=ObserverSchedulerDb;"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ObserverScheduler"
  },
  "CycleIntervalSeconds": 60
}
```

### 4. Run Database Migrations

```bash
dotnet ef database update
```

### 5. Build and Run

#### Using .NET CLI:

```bash
dotnet restore
dotnet build
dotnet run
```

#### Using Docker:

```bash
docker build -t observer-scheduler .
docker run -p 8080:80 observer-scheduler
```

## API Documentation

The API is documented using Swagger and is available at `/swagger` when running in development mode.

### Authentication

All API requests require an `api-key` header:

```
api-key: your-api-key-here
```

### User Management Endpoints

#### Create User (Admin Only)
```http
POST /user
Content-Type: application/json

{
  "userName": "john.doe",
  "role": "admin"  // or "stackholder"
}
```

#### Get User by ID
```http
GET /user/{id}
```

#### Delete User (Admin Only)
```http
DELETE /user/{id}
```

### Job Management Endpoints

#### Create Job
```http
POST /jobs
Content-Type: application/json

{
  "name": "DailyHealthCheck",
  "description": "Check API health daily",
  "cronExpression": "0 0 9 * * ?",
  "url": "https://api.example.com/health",
  "expireDate": "2025-12-31T23:59:59Z",
  "utcOffset": 3,
  "retryCount": 3,
  "retryDelay": 5000
}
```

#### Update Job
```http
PUT /jobs/{id}
Content-Type: application/json

{
  "id": "guid-here",
  "name": "UpdatedJobName",
  "description": "Updated description",
  "cronExpression": "0 0 12 * * ?",
  "url": "https://api.example.com/new-endpoint",
  "expireDate": "2026-12-31T23:59:59Z",
  "utcOffset": 0,
  "retryCount": 5,
  "retryDelay": 3000
}
```

#### Get Job Info
```http
GET /jobs/{id}
```

Response includes job details and last 10 execution logs.

#### Trigger Job Manually
```http
POST /jobs/{id}/trigger
```

#### Delete Job
```http
DELETE /jobs/{id}
```

## Job Model Properties

| Property | Type | Description |
|----------|------|-------------|
| `id` | Guid | Unique job identifier |
| `name` | string | Job name (must be unique per user) |
| `description` | string | Job description |
| `cronExpression` | string | Cron expression for scheduling |
| `url` | string | HTTP GET endpoint to execute (encrypted at rest) |
| `expireDate` | DateTime | Job expiration date |
| `utcOffset` | int | Timezone offset in hours |
| `retryCount` | int | Number of retry attempts on failure |
| `retryDelay` | int | Delay between retries in milliseconds |
| `userId` | Guid | Owner user ID |
| `logs` | List<JobLog> | Execution history (last 10 logs) |

## Security Features

### 1. API Key Authentication
- Users are authenticated via encrypted API keys stored in PostgreSQL
- API keys are generated using cryptographic random number generation

### 2. URL Encryption
- All job URLs are encrypted using AES-256-GCM before storage
- Each encrypted URL has a unique authentication tag
- Decryption happens only when retrieving job information

### 3. Role-Based Authorization
- **Admin**: Full access to user management and jobs
- **Stakeholder**: Access only to their own jobs

### 4. Tenant Isolation
- Each user has separate MongoDB collections for jobs and logs
- Format: `Jobs_{userId}` and `JobLogs_{userId}`

## Background Job Processing

The `JobBackgroundService` runs continuously with a configurable interval (default: 60 seconds):

1. Scans all active jobs across all tenants
2. Evaluates cron expressions against current time (with UTC offset)
3. Executes eligible jobs via HTTP GET requests
4. Logs execution results (success/failure with messages)
5. Implements retry logic for failed requests

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PostgreSQL connection string"
  },
  "MongoDbSettings": {
    "ConnectionString": "MongoDB connection string",
    "DatabaseName": "ObserverScheduler"
  },
  "CycleIntervalSeconds": 60  // Background service check interval
}
```

## Cron Expression Examples

| Expression | Description |
|------------|-------------|
| `0 0 9 * * ?` | Every day at 9:00 AM |
| `0 */15 * * * ?` | Every 15 minutes |
| `0 0 12 * * MON-FRI` | Weekdays at noon |
| `0 0 0 1 * ?` | First day of every month at midnight |
| `0 0 */6 * * ?` | Every 6 hours |

## Development

### Run Tests
```bash
dotnet test
```

### Build for Production
```bash
dotnet publish -c Release -o ./publish
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback to specific migration
dotnet ef database update PreviousMigrationName
```

## Docker Deployment

### Build Image
```bash
docker build -t observer-scheduler:latest .
```

### Run Container
```bash
docker run -d \
  -p 8080:80 \
  -v $(pwd)/cipher.txt:/app/cipher.txt \
  -e ConnectionStrings__DefaultConnection="your-postgres-connection" \
  -e MongoDbSettings__ConnectionString="your-mongo-connection" \
  observer-scheduler:latest
```

## Troubleshooting

### Common Issues

1. **Missing cipher.txt file**
   - Error: File not found exception
   - Solution: Create a 32-byte encryption key file in the project root

2. **Database connection failures**
   - Check connection strings in appsettings.json
   - Ensure PostgreSQL and MongoDB services are running
   - Verify firewall rules and network connectivity

3. **Jobs not executing**
   - Check cron expression syntax using online validators
   - Verify job expiration date is in the future
   - Check background service logs for errors
   - Ensure UTC offset is correctly configured

4. **Authentication errors**
   - Verify `api-key` header is included in requests
   - Check that the API key exists and is valid
   - Ensure user role has appropriate permissions

## License

This project is licensed under the MIT License.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Support

For issues, questions, or contributions, please open an issue in the GitHub repository.