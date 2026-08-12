TEMPLATE ACCESS CREDENTIALS:
username: root
password: 03Q@bD96GzWv

username: lxc
password: }fTT4cT08xw,

# SETUP GUIDE

## SETUP IN PROXMOX

### Download LXC Template

cd /var/lib/vz/template/cache
wget url

### Edit Linux Bridge

1. Go proxmox
2. Node (e.g: pve)
3. Network
4. Edit vmbr0

IPV4/CIDR: 192.168.122.1/24
Gateway: null

5. Ok

### Create Token

1. Go Proxmox
2. Select Datacenter
3. Permissions -> Api Tokens
4. Create API
5. Copy .env file

### Setup Proxy

I use pangolin for proxy.

pangolin.net -> Create User

I create a site.

I create a resources with ssh.

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

USE ```sudo docker compose up -d``` command

you can enter the website at: http://localhost:8000 

