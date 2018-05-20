#!/bin/bash

sed -i $2 -e "s/<Version>.*<\/Version>/<Version>1.0.$1<\/Version>/"