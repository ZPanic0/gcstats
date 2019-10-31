import protobuf from "protobufjs"

export default class Messages {
    static messages

    async get(messageName) {
        if (this.messages) {
            return this.messages.lookupType(messageName)
        } else {
            const subDomainModifier = process.env.GATSBY_IS_DEV ? "" : "/gcstats"
            const path = `${subDomainModifier}/all.proto`
            this.messages = await protobuf.load(path)
            return this.get(messageName)
        }
    }
}