import Messages from "./Messages"

export default class PlayerSource {
    static data
    static playerMessage

    async GetPlayer(lodestoneId) {
        if (!this.data) {
            this.data = []
        }

        const partitionId = lodestoneId % 1000
        const partition = this.data[partitionId]
        if (partition) {
            return partition.find((player) => player.LodestoneId === lodestoneId)
        } else {
            if (!this.playerMessage) {
                this.playerMessage = await new Messages().get("Players")
            }

            const filePath = `/players/${partitionId}.bin`
            const buffer = await (await fetch(filePath)).arrayBuffer()

            this.data[partitionId] = this.playerMessage.decode(new Uint8Array(buffer)).PlayerEntries
            return this.GetPlayer(lodestoneId)
        }
    }
}