rem creates a subnet so that the api service within docker container can access the sql running in it's own container under the same host
rem 192.168.0.1 is the ip of the host that the api uses to connect to sql
docker network create -d nat --subnet 192.168.0.0/24 --gateway 192.168.0.1 saas-net

docker run -d -p 4999:80 --name saas-api saas