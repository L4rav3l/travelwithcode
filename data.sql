sudo -u postgres psql

CREATE DATABASE travelwithcode;
\c travelwithcode;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    token_version INTEGER DEFAULT 1,
    username TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    salt TEXT NOT NULL,
    github_token TEXT,
    github_embedding TEXT,
    script TEXT,
    admin BOOLEAN DEFAULT FALSE,
    lxcId INTEGER DEFAULT 0,
);

INSERT INTO users (username, password, salt, admin) VALUES ('admin', '5F06lOOvXSdH9TY3xhH8/jVTBd8t6N4C7XDbxnVQaao=', 'zal9SGSihLh68ag8QsoVMg==', true);