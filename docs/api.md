# API reference

- [Login](#login)
- [Register](#register)
- [Check]()
- [Me](#me)
- [Change password]()
- [Change email]()
- [Delete user]()
- [Google auth]()

## Login

Url: `/api/auth/login`

Body:

```js
{
  email: string
  password: string
}
```

Status codes:

- 404 - user (email) isn't found
- 400 - password is incorrect
- 200 - ok

200 Response:

```js
{
  token: string
  email: string
}
```

## Register

Url: `/api/auth/register`

Body:

```js
{
  email: string
  password: string
  confirmPassword: string
}
```

Status codes:

- 400 - password is incorrect
- 409 - email is taken
- 500 - errors during registration
- 200 - ok

200 Response:

```js
{
  token: string
  email: string
}
```

## Check

Check if token is valid.

Url: `GET` `/api/auth/check`

Headers:

```
Authorization = your-token
```

## Me

## Change password

## Change email

## Delete user

## Google auth
