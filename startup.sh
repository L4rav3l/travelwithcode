sudo -u postgres psql

CREATE DATABASE travelwithcode;
\c travelwithcode;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username TEXT NOT NULL,
    password TEXT NOT NULL,
    salt TEXT NOT NULL,
    github_token TEXT,
    github_vector TEXT
);