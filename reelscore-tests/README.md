# ReelScore unit tests

Copy the `tests` directory into the ReelScore repository root, then run:

```bash
dotnet sln ReelScore.sln add tests/ReelScore.Api.Tests/ReelScore.Api.Tests.csproj
dotnet restore ReelScore.sln
dotnet test ReelScore.sln --configuration Release --collect:"XPlat Code Coverage"
```

The tests cover service mapping, normalization, missing-resource behavior, user conflicts, duplicate-rating prevention, rating creation, and rating updates.

If your repository interfaces differ from the signatures developed in the current refactor, run `dotnet build` and adjust only the affected mock setup signatures.
