sudo -u postgres psql

CREATE DATABASE travelwithcode;
\c travelwithcode;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    token_version INTEGER DEFAULT 1,
    username TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    hash TEXT NOT NULL,
    github_token TEXT,
    github_embedding TEXT,
    script TEXT,
    admin BOOLEAN DEFAULT FALSE,
    lxcId INTEGER DEFAULT 0,
);