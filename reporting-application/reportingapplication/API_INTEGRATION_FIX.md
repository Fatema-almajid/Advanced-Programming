# API Integration & Authorization Fix

## Problem Summary
The reporting application was unable to access API data, receiving **401 Unauthorized** errors because:
1. User role information was NOT being extracted from the login API response
2. The JWT token was not being included in API requests to the backend
3. The `ReportingController` had generic `[Authorize]` instead of role-based authorization

## Solutions Implemented

### 1. Enhanced LoginController.cs
**Changes Made:**
- Now extracts all three pieces of user information from the API response:
  - `token` - JWT token for API authentication
  - `userId` - User ID
  - `role` - User's role (INSTRUCTOR, TRAINING_COORDINATOR, TRAINEE, ADMIN)
- Stores these as claims in the authentication cookie:
  - `ClaimTypes.Email` - User email
  - `"Token"` - JWT token (custom claim)
  - `ClaimTypes.NameIdentifier` - User ID
  - `ClaimTypes.Role` - User role (**CRITICAL for authorization**)
- Added error handling for missing token
- Set proper expiration on authentication properties (1 hour)

**Key Code:**
```csharp
// Extract role from API response
if (root.TryGetProperty("role", out var roleElement))
{
    role = roleElement.GetString();
}

// Add role claim - critical for [Authorize(Roles = "...")] to work
if (!string.IsNullOrEmpty(role))
{
    claims.Add(new Claim(ClaimTypes.Role, role));
}
```

### 2. Updated ReportingController.cs
**Changes Made:**
- Changed from `[Authorize]` to `[Authorize(Roles = "INSTRUCTOR,TRAINING_COORDINATOR")]`
- Updated `FetchApiDataAsJson()` method to:
  - Extract JWT token from claims
  - Add token to HTTP request headers as Bearer token
  - Include proper logging for debugging
  - Log 401 errors specifically
- Added `System.Security.Claims` using statement

**Key Code:**
```csharp
// Get the JWT token from claims
var token = User?.FindFirst("Token")?.Value;

var request = new HttpRequestMessage(HttpMethod.Get, url);
if (!string.IsNullOrEmpty(token))
{
    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
}
```

### 3. Updated appsettings.json
**Added:**
```json
"AuthApi": {
  "Url": "https://localhost:7102/api/auth/login"
}
```
This provides a configurable endpoint for the authentication API (currently hardcoded but now available for configuration).

## How It Works Now

### Login Flow:
1. User enters credentials at `/Login/Login`
2. LoginController posts to `https://localhost:7102/api/auth/login`
3. API returns:
   ```json
   {
     "token": "eyJhbGc...",
     "userId": 5,
     "role": "INSTRUCTOR"
   }
   ```
4. LoginController extracts all three values and creates claims
5. **IMPORTANT:** Role claim is added, enabling role-based authorization
6. User is signed in with cookie containing the JWT token
7. Redirect to `/reporting` (ReportingController)

### Data Access Flow:
1. ReportingController's `[Authorize(Roles = "INSTRUCTOR,TRAINING_COORDINATOR")]` validates user has correct role
2. User's authentication cookie is validated
3. `GetReportData()` is called
4. For each API endpoint (users, courses, sessions, etc.):
   - Retrieve JWT token from claims
   - Create HTTP GET request
   - Add `Authorization: Bearer {token}` header
   - API validates the JWT token and returns data
5. Data is parsed and transformed into reporting format
6. View is returned with populated data

## Testing

### Prerequisites:
- Both applications running:
  - Reporting app: `https://localhost:7028` (or configured port)
  - API app: `https://localhost:7102`
- User account exists with role INSTRUCTOR or TRAINING_COORDINATOR

### Test Steps:
1. **Stop and restart the reporting application** (required - hot reload may not apply all changes)
2. Navigate to `https://localhost:7028/Login/Login`
3. Enter credentials for an INSTRUCTOR or TRAINING_COORDINATOR user
4. Click Login
5. Should be redirected to `/reporting` with data loaded
6. If you see data on dashboard, authorization is working!

### Troubleshooting:

| Issue | Cause | Solution |
|-------|-------|----------|
| Still getting 401 | Token not in request | Check browser DevTools Network tab, verify Bearer token in headers |
| "Unauthorized" after login | Role not extracted | Check LoginController is parsing API response correctly |
| Blank reporting data | API returning empty arrays | Verify API is running and has test data |
| Redirect loops | Cookie auth not working | Clear browser cookies and login again |

## Files Modified

1. ? `reportingapplication/Controllers/LoginController.cs` - Extract role from API response
2. ? `reportingapplication/Controllers/ReportingController.cs` - Include JWT token in API requests
3. ? `reportingapplication/appsettings.json` - Add AuthApi configuration
4. ? `reportingapplication/Program.cs` - Already configured correctly

## Important Notes

- The JWT token from the API is different from the cookie used for local authentication
- The cookie authenticates the user to the reporting app
- The JWT token authenticates the user to the backend API
- Both must be present and valid for the system to work
- Role must be included in claims for `[Authorize(Roles = "...")]` to work

## Next Steps

If you still encounter issues:
1. Check browser DevTools ? Network tab ? `/api/users` request
2. Look for `Authorization: Bearer ...` header
3. If header is missing, token is not being extracted correctly
4. If header is present but getting 401, check API's JWT validation
5. Review browser Console for any JavaScript errors
