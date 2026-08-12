TEMPLATE ACCESS CREDENTIALS:
username: root
password: 03Q@bD96GzWv

username: lxc
password: }fTT4cT08xw,

# SETUP GUIDE

## SETUP IN PROXMOX

### Download LXC Template
```
cd /var/lib/vz/template/cache
wget https://github.com/L4rav3l/travelwithcode/releases/download/1.0/travelwithcode.tar.zst
```

Edit Linux Bridge
Go to Proxmox.

Select Node (e.g., pve).

Go to Network.

Edit vmbr0:
IPv4/CIDR: 192.168.122.1/24
Gateway: leave empty / null

Click OK.

Create API Token
Go to Proxmox.

Select Datacenter.
Go to Permissions -> API Tokens.
Create an API Token.

Copy token credentials into your .env file.

### Setup Proxy

Setup Proxy
(I use Pangolin for the proxy, but you can choose another tool.)

Create an account at proxmox.net.
Create sites and set up an LXC container.
When creating sites, set up the required resources via SSH.

## Setup frontend & backend

### Create .ENV file:

content:

```
POSTGRES_USER=postgres
POSTGRES_PASSWORD=jelszod
POSTGRES_DB=scraperdb
JWT_SECRET=dsfghjafdhadfsghjkadfghjkadfghjks
PSQL_CONNECTION="Host=localhost;Port=5432;Username=postgres;Password=jelszod;Database=travelwithcode;Pooling=true;MaxPoolSize=100;Timeout=5;"
PRIVATE_KEY=HEINER_ERNO_1234

PROXMOX_URL=
PROXMOX_TOKEN=
PROXMOX_DATACENTER=pve
```
You don't need to install PostgreSQL or any other dependencies, as Docker takes care of that.

USE ```sudo docker compose up -d``` command

you can enter the website at: http://localhost:8000 

