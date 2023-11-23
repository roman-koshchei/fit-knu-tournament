# QuasarSoft Auth

Authentication service made by QuasarSoft team for FIT KNU Tournament 2023

## API tasks

- [x] Registration: POST email + pasword + confirm password -> return token
- [x] Login: POST email + password -> return token
- [x] Check: GET token -> ok if token valid
- [x] ME: GET token -> email
- [x] Change password: POST token + old password + new password + change user version +1
- [x] Change email: POST token + email + change user version + 1
- [x] Delete user: DELETE token + password
- [x] Google

## UI tasks

- [x] Home/login form/page
- [x] Registartion page
- [x] Account info
- [x] Confirm account delete page
- [x] Change password page
- [x] Change email page
- [ ] Google

## API reference

To work with API you can look at:

- Swagger documentation on the [website](https://fit-knu-tournament.onrender.com/swagger)
- Markdown documentation [API reference](./docs/api.md)

## Registration

Endpoint: `POST /api/auth/register`

### Request
```
{
  email: user@exmple.com,
  password: your_password,
  confirmPassword: your_password
}
```

### Response
```
{
  status_code: 200,
  token: your_access_token
}
```

Posible status codes:
- 200 OK: Registration was successful.
- 400 Bad Request: Data is unvaliable.
- 500 Internal Server Error: Registration failed.

## Login

Endpoint: `POST /api/auth/login`

### Request
```
{
  email: user@example.com,
  password: your_password
}
```

### Response
```
{
  status_code: 200,
  token: your_access_token
}
```

Possible status codes:
- 200 OK: Succesful login.
- 401 Unauthorized: Incorrect credentials

## Check

Endpoint: `GET /api/autrh/check`

### Request
```
{
  token: your_token
}
```

### Response
```
{
  status_code: 200
}
```

Possible status codes:
- 200 OK: Token is valid
- 401 Unauthorized: Token is invalid

## Me

Endpoint: `GET /api/auth/me`

### Request:
```
{
  token: your_token
}
```

### Response
```
{
  email: user@example.com
}
```

Possible status codes:
- 200 OK: Token is valid, and user information returned.
- 401 Unauthorized: Token is invalid.

## Change Password

Endpoint: `POST /api/auth/change-password`

### Request
```
{
  token: your_access_token,
  oldPassword: your_old_password,
  newPassword: your_new_password
}
```

### Response
```
{
  status_code: 200
}
```

Possible status code:
- 200 OK: Successful password change.
- 401 Unauthorized: Invalid old password.

## Change Email

Endpoint: `POST /api/auth/change-email`

### Request
```
{
  token: your_access_token,
  email: new_email@gmail.com
}
```

### Response
```
{
  status_code: 200
}
```

Possible status codes:
- 200 OK: Successful email change.
- 401 Unauthorized: Invalid token.

## Delete User

Endpoint: `DELETE /api/auth/delete-user`

### Request
```
{
  token : your_access_token,
  password: your_password
}
```

### Response
```
{
  status_code: 200
}
```

Possible status codes:
- 200 OK: Successful user deletion.
- 401 Unauthorized: Invalid password.

## Google Auth

Endpoint: `POST /api/auth/google`

### Request
```
{
  token: your_google_access_token
}
```

### Response
```
{
  status_code: 200,
  token: your_access_token
}
```

Possible status codes:
- 200 OK: Successful Google authentication
- 401 Unauthorized: Invalid Google access token
