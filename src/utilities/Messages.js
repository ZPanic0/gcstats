import protobuf from "protobufjs"

export default class Messages {
    static messages

    async get(messageName) {
        if (this.messages) {
            return this.messages.lookupType(messageName)
        } else {
            this.messages = await protobuf.load("/all.proto")
            return this.get(messageName)
        }
    }
}