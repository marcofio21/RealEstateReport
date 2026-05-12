# Real Estate Report

## Overview

This application retrieves real estate listings from a Partner API, aggregates the data by real estate agent, and produces a ranking of the most active agents based on the number of listings.

The goal of this project is to demonstrate clean architecture, separation of concerns, and practical handling of external API limitations such as rate limiting and pagination.

---

## Features

- Retrieves real estate listings from an external Partner API
- Aggregates listings by real estate agent (`MakelaarId`)
- Calculates agent activity based on number of listings
- Supports filtering by:
  - Listing type (e.g. sale/rent)
  - Location(s)
  - Optional garden filter
- Produces ranked results of most active real estate agents
- Handles paginated API responses and aggregates data across pages
- Applies rate limiting to respect external API constraints

---

## Application

A simple console application is provided to execute the service and display results in a readable tabular format for demonstration purposes.

---

## Architecture

The solution is structured into the following layers:

- **Client Layer**
  - `PartnerApiClient`
  - Responsible for HTTP communication with the external API

- **Service Layer**
  - `RealEstateAgentService`
  - Contains business logic for aggregation and ranking

- **Configuration**
  - `PartnerApiSettings` (bound from `appsettings.json` using `IOptions<T>`)

- **Models / DTOs**
  - API request/response models
  - Domain models such as `RealEstateAgent`
  - Options models:
    - `RealEstateAgentRankingOptions` (service-level contract)
    - `ListingApiRequestOptions` (API-level contract)

---

## Technical Decisions

### HttpClientFactory
The application uses `HttpClientFactory` to ensure efficient and reusable HTTP connections.

### Rate Limiting
A `FixedWindowRateLimiter` is used to prevent exceeding the API limit (~100 requests/minute).

### Pagination Handling
All pages are retrieved sequentially and aggregated in-memory to ensure correct ranking results.

### Aggregation Strategy
Listings are grouped by `MakelaarId`, and the number of listings per agent is used to determine ranking.

---
## Possible Improvements

- Add a persistent caching layer to reduce API calls and improve performance
- Introduce a retry policy (e.g. exponential backoff) for more resilient API communication
- Improve observability with structured logging instead of console-based output
- Add unit and integration tests for the service and API client layers
- Move console output into a dedicated reporting/export layer (e.g. CSV or JSON exporter)

---

## Author

Built as a technical assignment to demonstrate:

- .NET 8 development practices
- External API integration
- Clean architecture principles with separation of concerns
- Pragmatic handling of external constraints (rate limiting, pagination, API reliability)

---

## Configuration

The API configuration is stored in `appsettings.json`:

```json
{
  "PartnerApi": {
    "BaseUrl": "http://partnerapi.funda.nl/feeds/Aanbod.svc/",
    "Key": "YOUR_API_KEY"
  }
}