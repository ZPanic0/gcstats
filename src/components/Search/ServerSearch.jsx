import React, { Component } from "react"
import { Dropdown } from "semantic-ui-react"
import Servers from "../../utilities/Servers"

export default class ServerSearch extends Component {
    constructor(props) {
        super(props)

        this.servers = Servers
            .slice(1)
            .map((server) => { return { key: server, value: server, text: server } })
    }

    render() {
        return (
            <Dropdown
                onChange={this.props.handleChange}
                options={this.servers}
                placeholder="Select Server..."
                selection
                search
                value={this.props.value}
            />
        )
    }
}