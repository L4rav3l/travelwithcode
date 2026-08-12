TEMPLATE ACCESS CREDENTIALS:
- Username: root | Password: 03Q@bD96GzWv
- Username: lxc  | Password: }fTT4cT08xw,

# SETUP GUIDE

## 1. PREREQUISITES & DOCKER INSTALLATION

You do not need to manually install PostgreSQL, Node.js, or any database tools. Docker Compose automatically handles all required services and dependencies.

However, you must have Docker and Docker Compose installed on your system. 

If you haven't installed Docker yet, run the following commands (Ubuntu/Debian):

```bash
# Update package index and install Docker + Docker Compose plugin
sudo apt update
sudo apt install -y docker.io docker-compose-plugin

# Start and enable Docker service
sudo systemctl start docker
sudo systemctl enable docker
```

# 2. PROXMOX & LXC SETUP

## Download LXC Template
Log into your Proxmox server via SSH and run:

```bash
cd /var/lib/vz/template/cache
wget [https://github.com/L4rav3l/travelwithcode/releases/download/1.0/travelwithcode.tar.zst](https://github.com/L4rav3l/travelwithcode/releases/download/1.0/travelwithcode.tar.zst)
```

## Edit Linux Bridge

1. Open the Proxmox Web UI.
2. Select your Node (e.g., pve).
3. Go to Network.
4. Edit vmbr0:
  - IPv4/CIDR: 192.168.122.1/24
  - Gateway: (Leave empty / null)
5. Click OK and apply network configuration.

## Create API Token

1. In Proxmox Web UI, go to Datacenter (top level).
2. Go to Permissions -> API Tokens.
3. Click Add to create a new API Token.
4. Copy the generated Token ID and Secret Value (you will need these for your .env file).

## Setup Proxy (Pangolin)
(Pangolin is used in this setup, but you can use any other proxy manager.)

1. Create an account at proxmox.net.
2. Create your site and configure an LXC container.
3. Connect to your server via SSH to complete the proxy configuration and link the resources.

# 3. FRONTEND & BACKEND SETUP

## Create .env File
Create a .env file in the root directory of the project with the following content. Make sure to fill in your Proxmox credentials!

```
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=scraperdb
JWT_SECRET=dsfghjafdhadfsghjkadfghjkadfghjks
PSQL_CONNECTION="Host=localhost;Port=5432;Username=postgres;Password=your_secure_password;Database=travelwithcode;Pooling=true;MaxPoolSize=100;Timeout=5;"
PRIVATE_KEY=HEINER_ERNO_1234

# Proxmox Configuration (REQUIRED)
PROXMOX_URL=https://192.168.1.100:8006
PROXMOX_TOKEN=PVEAPIToken=USER@PAM!TOKENID=UUID
PROXMOX_DATACENTER=pve
```

## Run the Application
Start all containerized services using Docker Compose:

```bash
sudo docker compose up -d
```

Once the containers are running, you can access the website at:
http://localhost:8000
