param (
    $LocalTagName,
    $RemoteTagName,
    $Version
)

#az acr login --name volocr

docker tag $LocalTagName":"$version $RemoteTagName":"$version
docker push $RemoteTagName":"$version
