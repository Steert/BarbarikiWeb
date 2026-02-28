# Barbariki Delivery Management System

### This project is a delivery management system built with ASP.NET Core (Backend) and React (Frontend), utilizing PostgreSQL for data storage. The entire stack is containerized using Docker for easy deployment and local development.

## Configure Environment Variables

### Create a .env file in the root directory and provide the following configuration:

##### Database Configuration
POSTGRES_USER=u97l6ri1th8o6f
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=d7vt9376nlp65q

##### Backend Connection String
DATABASE_STRING_KEY=Host=db;Database=d7vt9376nlp65q;Username=u97l6ri1th8o6f;Password=your_secure_password

##### Frontend Build Arguments
VITE_API_URL=http://localhost:5000/orders
VITE_MAP_BOX_TOKEN=your_mapbox_token_here

Descriptions: why we use cloud database - we thought we could deploy the app but we didn't have enough time for this. 
