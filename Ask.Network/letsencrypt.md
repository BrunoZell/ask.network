# Letsencrypt DNS Challenge

Run certbot DNS challenge for ask.network (remove `--staging`)

```bash
sudo docker run -it --rm -v /docker-volumes/etc/letsencrypt:/etc/letsencrypt -v /docker-volumes/var/lib/letsencrypt:/var/lib/letsencrypt -v "/docker-volumes/var/log/letsencrypt:/var/log/letsencrypt" certbot/certbot certonly --manual --preferred-challenges=dns -d "ask.network" --email "bruno@ask.network" --staging
```

Run certbot DNS challenge for claims.ask.network (remove `--staging`)

```bash
sudo docker run -it --rm -v /docker-volumes/etc/letsencrypt:/etc/letsencrypt -v /docker-volumes/var/lib/letsencrypt:/var/lib/letsencrypt -v "/docker-volumes/var/log/letsencrypt:/var/log/letsencrypt" certbot/certbot certonly --manual --preferred-challenges=dns -d "claims.ask.network" --email "bruno@ask.network" --staging
```

Then copy certificates from:

```bash
sudo ls /docker-volumes/etc/letsencrypt/live/ask.network
sudo ls /docker-volumes/etc/letsencrypt/live/claims.ask.network
```
