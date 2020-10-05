#!/bin/zsh

dotnet mpc -i ../Assets/_Scripts/LockStep/_Header -o ../Assets/_Scripts/LockStep/Generated.cs

if [ -d "../LockstepServer/_Header" ] 
then
    rm -rf ../LockstepServer/_Header
fi
cp -R ../Assets/_Scripts/LockStep/_Header ../LockstepServer/_Header

if [ -d "../LockstepServer/_Common" ] 
then
    rm -rf ../LockstepServer/_Common
fi
cp -R ../Assets/Sources/_Common ../LockstepServer/_Common

cp ../Assets/_Scripts/LockStep/Generated.cs ../LockstepServer/MessagePack/Generated.cs

cp ../Assets/3rd_party/SLikeNet/DefaultMessageIDTypes.cs ../LockstepServer/SLikeNet/
