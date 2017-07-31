# Fidget.Validation.Addresses
Address validation and metadata exploration via the Google Address Data Service.
This is a third-party library that has no affiliation with Google.

Release notes for **1.0.0-alpha2**:
- Address validation not yet implemented.
- Metadata exploration mostly complete.

## Usage
Install the package from [NuGet](https://www.nuget.org/packages/Fidget.Validation.Addresses/).

Using the service in your application can be accomplished three ways:
1. Use `AddressService.Default` directly.
2. Map the `IAddressService` interface to `AddressService.Default` in your DI container.
3. Map the `IAddressService` interface to `AddressService.FactoryMethod()` in your DI container.

### Metadata Exploration
There are five methods for exploring metadata:
- `GetGlobalAsync`, which contains the list of country keys.
- `GetCountryAsync`, which returns country information and lists its contained provinces.
- `GetProvinceAsync`, which returns province information and lists its contained localities.
- `GetLocalityAsync`, which returns locality information and lists its contained sublocalities.
- `GetSublocalityAsync`, which returns sublocality information.

Synchronous versions of these methods are available as extension methods.

At this time, the `cancellationToken` argument is unused, but is intended to be implemented in a future release.

The [metadata classes](https://github.com/seanterry42/Fidget.Validation.Addresses/tree/master/src/Metadata) contain
field documentation.

## License
Copyright 2017 Sean Terry

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.