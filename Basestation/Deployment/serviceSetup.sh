
#!/bin/bash

# to run: $ sudo bash ./serviceSetup.sh

# exit when any command fails
set -e

# include parse_yaml function
. parse_yaml.sh

# must run as root
if (( $EUID != 0 )); then
    echo "Please run as root"
    exit
fi

# read yaml file
echo "parsing system configuration..."
eval $(parse_yaml init_conf.yml "config_")

# stop and remove all present containers
if [ "$(docker ps -aq | grep -c "^$1")" -ge 1 ]; then
    echo "present containers found"
    echo "stoping container IDs:"
    docker stop $(docker ps -a -q)
    echo "removing container IDs:"
    docker rm $(docker ps -a -q)
fi

# remove network to be used
if [ "$(docker network ls -q -f name=$config_setup_network_name)" ]; then
    echo $config_setup_network_name "exists. Removing."
    docker network rm $config_setup_network_name
fi  

# remove old tag-less "dangling" images
if [ "$(docker images -f "dangling=true" -q | grep -c "^$1")" -ge 1 ]; then
    echo "found dangling images - removing to save memory"
    docker rmi $(docker images --filter "dangling=true" -q --no-trunc)
fi

# TODO: ADD REMOVE CODE HERE!

# set system variables
networkName=$config_setup_network_name
subnet=$config_setup_network_subnet

SHMimage=$config_setup_services_SHM_image
SHMname=$config_setup_services_SHM_name
SHMip=$config_setup_services_SHM_ip

MACSimage=$config_setup_services_MACS_image
MACSname=$config_setup_services_MACS_name
MACSip=$config_setup_services_MACS_ip

SDAS1image=$config_setup_services_SDAS1_image
SDAS1name=$config_setup_services_SDAS1_name
SDAS1ip=$config_setup_services_SDAS1_ip

SDAS2image=$config_setup_services_SDAS2_image
SDAS2name=$config_setup_services_SDAS2_name
SDAS2ip=$config_setup_services_SDAS2_ip

# load images
echo "loading" $SHMimage "image"
docker load < $SHMimage.tar
echo "loading" $MACSimage "image"
docker load < $MACSimage.tar
echo "loading" $SDAS1image "image"
docker load < $SDAS1image.tar
echo "loading" $SDAS2image "image"
docker load < $SDAS2image.tar


# create network
echo "creating network:" $config_setup_network_name "with subnet:" $config_setup_network_subnet
docker network create -d bridge --subnet $subnet $networkName
#echo $config_setup_network_name "created"

# run containers
echo "running containers..."
docker run -d --net $networkName --ip $SHMip --restart always --name $SHMname $SHMimage #shm is service for tests
docker run --net $networkName --ip $MACSip --restart always --name $MACSname $MACSimage
docker run --net $networkName --ip $SDAS1ip --restart always --name $SDAS1name $SDAS1image
docker run --net $networkName --ip $SDAS2ip --restart always --name $SDAS2name $SDAS2image
echo ""
echo "Done. Listing containers:"
echo ""
docker ps

# TODO: Add error checking (on all loading/creating)!
