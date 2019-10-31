import Messages from "./Messages"

export default class PlayerData {
    static data
    static playerMessage

    async GetPlayer(lodestoneId) {
        this.data = this.data || []
        this.playerMessage = this.playerMessage || await new Messages().get("Players")

        const partitionId = lodestoneId % 1000
        const partition = this.data[partitionId]
        
        if (partition) {
            return partition.find((player) => player.LodestoneId === lodestoneId)
        } else {
            const subDomainModifier = process.env.GATSBY_IS_DEV ? "" : "/gcstats"
            const buffer = await (await fetch(`${subDomainModifier}/players/${partitionId}.bin`)).arrayBuffer()

            this.data[partitionId] = this.playerMessage.decode(new Uint8Array(buffer)).PlayerEntries

            return this.GetPlayer(lodestoneId)
        }
    }
}