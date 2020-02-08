from PIL import Image
import math
import sys

def main(mapPath, mapWidth, mapHeight, outputPath):
	img = Image.open(mapPath)

	widthPixel = img.size[0]
	heightPixel = img.size[1]

	CONST_WIDTH_SIZE = mapWidth
	CONST_HEIGHT_SIZE = mapHeight

	width = math.floor(widthPixel / CONST_WIDTH_SIZE)
	height = math.floor(heightPixel / CONST_HEIGHT_SIZE)

	widthRemainOne = math.ceil(widthPixel / CONST_WIDTH_SIZE) > width
	heightRemainOne = math.ceil(heightPixel / CONST_HEIGHT_SIZE) > height

	extensionStr = mapPath.split('/')[-1].split('.')[-1]
	for x in range(0, width):
		for y in range(0, height):
			fileName = "{}{}_{}.{}".format(outputPath, x, y, extensionStr)
			temp = img.crop(
				(
					x * CONST_WIDTH_SIZE,
					heightPixel - (y + 1) * CONST_HEIGHT_SIZE,
					x * CONST_WIDTH_SIZE + CONST_WIDTH_SIZE,
					heightPixel - (y + 1) * CONST_HEIGHT_SIZE + CONST_HEIGHT_SIZE
				)
			)
			temp.save(fileName)

	if widthRemainOne:
		x = width
		for y in range(0, height):
			fileName = "{}{}_{}.{}".format(outputPath, x, y, extensionStr)
			temp = img.crop(
				(
					x * CONST_WIDTH_SIZE,
					heightPixel - (y + 1) * CONST_HEIGHT_SIZE,
					x * CONST_WIDTH_SIZE + CONST_WIDTH_SIZE,
					heightPixel - (y + 1) * CONST_HEIGHT_SIZE + CONST_HEIGHT_SIZE
				)
			)
			temp.save(fileName)

	if heightRemainOne:
		y = height
		for x in range(0, width):
			fileName = "{}{}_{}.{}".format(outputPath, x, y, extensionStr)
			temp = img.crop(
				(
					x * CONST_WIDTH_SIZE,
					heightPixel - (y + 1) * CONST_HEIGHT_SIZE,
					x * CONST_WIDTH_SIZE + CONST_WIDTH_SIZE,
					heightPixel - (y + 1) * CONST_HEIGHT_SIZE + CONST_HEIGHT_SIZE
				)
			)
			temp.save(fileName)

	if widthRemainOne and heightRemainOne:
		x = width
		y = height

		fileName = "{}{}_{}.{}".format(outputPath, x, y, extensionStr)
		temp = img.crop(
			(
				x * CONST_WIDTH_SIZE,
				heightPixel - (y + 1) * CONST_HEIGHT_SIZE,
				x * CONST_WIDTH_SIZE + CONST_WIDTH_SIZE,
				heightPixel - (y + 1) * CONST_HEIGHT_SIZE + CONST_HEIGHT_SIZE
			)
		)
		temp.save(fileName)

if __name__ == "__main__":
	mapPath = sys.argv[1]
	mapWidth = int(sys.argv[2])
	mapHeight = int(sys.argv[3])
	outputPath = sys.argv[4]

	main(mapPath, mapWidth, mapHeight, outputPath)