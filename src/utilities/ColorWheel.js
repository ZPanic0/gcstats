export default class ColorWheel {
    red = 198
    blue = 198
    green = 198
    step = 33
    stepCount = 0
    minimum = 99
    maximum = 198
    rgbCap = 255

    getNext() {
        switch (this.stepCount) {
            case 0:
                this.red -= 33
                this.stepCount++
                break
            case 1:
                this.blue -= 33
                this.stepCount++
                break
            case 2:
                this.red += 33
                this.stepCount++
                break
            case 3:
                this.green -= 33
                this.stepCount++
                break
            case 4:
                this.blue += 33
                this.stepCount++
                break
            case 5:
                this.red -= 33
                this.stepCount++
                break
            case 6:
                this.blue -= 33
                this.stepCount = 0
                return this.getNext()
            default:
                throw Error("ColorWheel stepCount out of bounds")
        }

        this.red = this.red % this.rgbCap < this.minimum ? this.maximum : this.red % this.rgbCap
        this.green = this.green % this.rgbCap < this.minimum ? this.maximum : this.green % this.rgbCap
        this.blue = this.blue % this.rgbCap < this.minimum ? this.maximum : this.blue % this.rgbCap

        return `rgb(${this.red},${this.green},${this.blue})`
    }
}