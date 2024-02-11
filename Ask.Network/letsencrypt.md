# Letsencrypt DNS Challenge

Run certbot DNS challenge for ask.network (remove `--staging`)

```bash
sudo docker run -it --rm -v /docker-volumes/etc/letsencrypt:/etc/letsencrypt -v /docker-volumes/var/lib/letsencrypt:/var/lib/letsencrypt -v "/docker-volumes/var/log/letsencrypt:/var/log/letsencrypt" certbot/certbot certonly --manual --preferred-challenges=dns -d "ask.network" --email "bruno@ask.network" --staging
```

Run certbot DNS challenge for claims.ask.network (remove `--staging`)

```bash
sudo docker run -it --rm -v /docker-volumes/etc/letsencrypt:/etc/letsencrypt -v /docker-volumes/var/lib/letsencrypt:/var/lib/letsencrypt -v "/docker-volumes/var/log/letsencrypt:/var/log/letsencrypt" certbot/certbot certonly --manual --preferred-challenges=dns -d "claims.ask.network" --email "bruno@ask.network" --staging
```

NGINX is configured to read the certificates from the docker volume at /docker-volumes:

```yaml
services:
  nginx-production:
    volumes:
    - /docker-volumes/etc/letsencrypt/live/ask.network/fullchain.pem:/etc/letsencrypt/live/ask.network/fullchain.pem
    - /docker-volumes/etc/letsencrypt/live/ask.network/privkey.pem:/etc/letsencrypt/live/ask.network/privkey.pem
```

You can start all webservices again via `docker compose up -d` in this directory.

For backup purposes, you can view and copy the new certificates from this location

```bash
sudo ls /docker-volumes/etc/letsencrypt/live/ask.network
sudo ls /docker-volumes/etc/letsencrypt/live/claims.ask.network
```

Using the commands:

```bash
sudo cp -r /docker-volumes/etc/letsencrypt/archive/* ~/letsencrypt-backup/
sudo ls -l ~/letsencrypt-backup/
```
