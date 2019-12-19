# OBS!!!
# After reboot qemu may have to be restarted
# In that case the builder will loose the qemu connection and have to be recreated
# Run the following:
# docker buildx rm newbuilder (or whatever name the builder has, list with docker buildx ls)
# docker run --rm --privileged multiarch/qemu-user-static --reset -p yes
# docker buildx create --name newbuilder
# docker buildx use newbuilder
# docker buildx inspect --bootstrap

# Image names must reflect image in init_config.yml
MACS_IMAGE=macs

build:
	# building without parameter is TBD
	# Use 'make x64' or 'make arm'

arm:
	# publish MACS project for arm
	cd Basestation.MobileAppCommunication; dotnet publish -r linux-arm -c Release -o output/arm
	# Build MACS docker image
	cd Basestation.MobileAppCommunication; docker buildx build --no-cache=true --build-arg dir=arm --platform linux/arm/v7 -t $(MACS_IMAGE) --load .
	# save MACS image as .tar
	docker save -o Deployment/macs.tar $(MACS_IMAGE)
	# remove image from development system
	docker image rm macs

x64:
	# Publish MACS project and build docker for local execution
	cd Basestation.MobileAppCommunication; dotnet publish -c Release -o output/x64
	cd Basestation.MobileAppCommunication; docker build --no-cache=true --build-arg dir=x64 -t $(MACS_IMAGE) .
	

clean:
	# Clean-up is TBD

test:
	#@echo test $(filter-out $@,$(MAKECMDGOALS))
	var=$(filter-out $@,$(MAKECMDGOALS))
	


