# TrailerDownloader

## Description
TrailerDownloader is an application that downloads missing trailers for movies in your Plex library and places them in the same folder as the movie with a specific naming convention. The application has been overhauled to use .NET 8 for the backend and React with TypeScript for the frontend.

## System Requirements
- .NET 8 SDK
- Node.js (for React frontend)

## Installation and Setup

### Backend (.NET 8 API)
1. Clone the repository and switch to the `fresh-start` branch.
2. Navigate to the `TrailerDownloaderAPI` directory.
3. Run `dotnet restore` to install backend dependencies.
4. Run `dotnet run` to start the API server.

### Frontend (React with TypeScript)
1. Navigate to the `clientapp` directory.
2. Run `npm install` to install frontend dependencies.
3. Run `npm start` to start the React development server.

## API Endpoints
- Search for movies: `GET /Movies/search?query={query}`
- Download movie trailers: `GET /Movies/download/{movieId}`

## Configuration
- The TMDB API key should be set in the `appsettings.json` file under the `TMDB_API_KEY` property.

## Docker Usage
The Docker setup has been updated to work with the new .NET 8 backend and React frontend. Instructions will be provided soon.

## Demo
The demo will be updated to showcase the new React frontend and .NET 8 backend functionality.

## Structure
The expected structure for movie directories remains the same as the previous version of the application.

## Donate
If you find this application useful, consider buying me a cup of coffee to support its continued development!

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ZRP9ZGW3RDDRN)

Enjoy the new and improved TrailerDownloader!
