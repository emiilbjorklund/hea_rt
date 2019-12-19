# Docker Installation Guide

1. Installation guide for Docker in a Linux development environment, to enable building cross-platform images.
2. Guide for using docker images on RaspberryPi


## 1. Install docker 19.03+ to get buildx : 

This guide assumes Ubuntu 18.04 or higher.  
Follow step by step and execute commands. 


- `sudo apt-get remove docker docker-engine docker.io`

- `sudo apt-get update`

- `sudo apt-get install apt-transport-https ca-certificates curl software-properties-common`

- `curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -`

- `sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu  $(lsb_release -cs)  stable"`

- `sudo apt-get update`  

Verify installation
- `docker --version`  

For more thorough information and guidance the following link might help.
https://phoenixnap.com/kb/how-to-install-docker-on-ubuntu-18-04

### Enable cross-platform build capabilities

#### Get qemu-user-static 

- `uname -m`

- `docker run --rm -t arm64v8/ubuntu uname -m`

- `docker run --rm --privileged multiarch/qemu-user-static --reset -p yes`  

Verify installation, should output: aarch64
- `docker run --rm -t arm64v8/ubuntu uname -m`


#### Create builder

- `docker buildx create --name newbuilder`

- `docker buildx use newbuilder`

- `docker buildx inspect --bootstrap`


#### Observe regarding reboot issues
After reboot qemu may have to be restarted.  
In that case the builder will loose the qemu connection and have to be recreated. Run the following:
- `docker buildx rm newbuilder`
- `docker run --rm --privileged multiarch/qemu-user-static --reset -p yes`
- `docker buildx create --name newbuilder`
- `docker buildx use newbuilder`
- `docker buildx inspect --bootstrap`

## 2. Install docker on Raspberry Pi
- `curl -sSL https://get.docker.com | sh`

- `sudo usermod -aG docker pi`  

Verify installation
- `docker run hello-world` 







