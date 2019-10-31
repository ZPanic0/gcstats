import Messages from "./Messages"

export default class PlayerIndex {
    static data
    static indexMessage

    async GetIndex(server) {
        this.data = this.data || []
        this.indexMessage = this.indexMessage || await new Messages().get("Indexes")

        const serverList = this.data[server]
        
        if (serverList) {
            return serverList
        } else {
            const subDomainModifier = process.env.GATSBY_IS_DEV ? "" : "/gcstats"
            const buffer = await (await fetch(`${subDomainModifier}/indexes/${server}.bin`)).arrayBuffer()

            this.data[server] = this.indexMessage.decode(new Uint8Array(buffer)).IndexEntries

            return this.GetIndex(server)
        }
    }
}