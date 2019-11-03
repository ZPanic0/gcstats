import React, { Component } from "react"
import { Message } from "semantic-ui-react"

export default class MessageBlock extends Component {
    render() {
        return (
            <div>
                <Message negative>
                    <Message.Header>This site hasn't launched yet.</Message.Header>
                    <p>Data here is not being updated yet. Results may be stale and bugs are probably present.</p>
                </Message>
            </div>
        )
    }
}