# IntelliBot Configuration Setup

## Security Notice
This project uses sensitive API keys that should not be committed to version control. Follow the setup instructions below to configure your local environment.

## Configuration Setup

### Option 1: Using Template Files (Recommended)

1. Copy the template files to create your actual configuration:
   ```powershell
   Copy-Item appsettings.Development.template.json appsettings.Development.json
   Copy-Item appsettings.Production.template.json appsettings.Production.json
   ```

2. Edit the copied files and replace the placeholder values:
   - `YOUR_AZURE_SPEECH_SUBSCRIPTION_KEY` - Your Azure Speech Services subscription key
   - `YOUR_INTELLIGUIDE_API_KEY` - Your IntelliGuide API key
   - `YOUR_OPENAI_API_KEY` - Your OpenAI API key
   - `YOUR_PRODUCTION_API_ADDRESS` - Your production API address

### Option 2: Using User Secrets (Development Only)

For development, you can use ASP.NET Core User Secrets:

1. Initialize user secrets:
   ```powershell
   dotnet user-secrets init
   ```

2. Set your secrets:
   ```powershell
   dotnet user-secrets set "AzureSpeech:SubscriptionKey" "your-azure-key"
   dotnet user-secrets set "intelliGuide:ApiKey" "your-intelliguide-key"
   dotnet user-secrets set "OpenAI:ApiKey" "your-openai-key"
   ```

### Option 3: Using Environment Variables

Set the following environment variables:
- `AzureSpeech__SubscriptionKey`
- `intelliGuide__ApiKey`
- `OpenAI__ApiKey`

## Required API Keys

### Azure Speech Services
1. Go to [Azure Portal](https://portal.azure.com)
2. Create a Speech Services resource
3. Copy the subscription key and region

### OpenAI API
1. Go to [OpenAI Platform](https://platform.openai.com)
2. Create an API key
3. Copy the key

### IntelliGuide API
Contact your system administrator for the API key and endpoint.

## Important Notes

- **Never commit actual API keys to version control**
- The `.gitignore` file excludes `appsettings.*.json` files
- Always use template files or environment variables in production
- Keep your API keys secure and rotate them regularly
