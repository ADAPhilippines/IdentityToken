#!/bin/bash
#!/bin/sh
mkdir -p dist/dotnet6
curl -sSL https://dot.net/v1/dotnet-install.sh > ./dist/dotnet-install.sh
chmod +x ./dist/dotnet-install.sh
./dist/dotnet-install.sh -c 6.0 -InstallDir ./dist/dotnet6
./dist/dotnet6/dotnet --version
./dist/dotnet6/dotnet publish -c Release -o ./dist src/IdentityToken.UI.WASM/IdentityToken.UI.WASM.csproj