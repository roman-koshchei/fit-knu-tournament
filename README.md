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
- [ ] Google:

## UI tasks

- [x] Home/login form/page
- [x] Registartion page
- [x] Account info
- [x] Confirm account delete page
- [ ] Change password page
- [ ] Change email page

## API reference

To work with API you can look at:

- Swagger documentation on the [website](https://fit-knu-tournament.onrender.com/swagger)
- Markdown documentation [API reference](./docs/api.md)
