[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Latest GitHub release](https://img.shields.io/github/v/release/hmlendea/transliteration-api)](https://github.com/hmlendea/transliteration-api/releases/latest)

# About

REST API for transliterating text into the latin alphabet, with support for over 35 languages.

# Usage

## Building from source

Just run the following command:
```sh
dotnet build
```

## Running

Just run the following command:
```sh
dotnet run
```

## Example request

The following example will transliterate the Russian name _Экваториальная Африка_ into _Ekvatorialnaya Afrika_:
```sh
curl --insecure --request GET --location 'http://localhost:5000/Transliteration?text=%D0%AD%D0%BA%D0%B2%D0%B0%D1%82%D0%BE%D1%80%D0%B8%D0%B0%D0%BB%D1%8C%D0%BD%D0%B0%D1%8F%20%D0%90%D1%84%D1%80%D0%B8%D0%BA%D0%B0&language=ru'
```
