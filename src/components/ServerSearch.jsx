import React from 'react'
import { Dropdown } from 'semantic-ui-react'

export default class ServerSearch extends React.Component {
    render() {
        return(<Dropdown
            placeholder='Select Server'
            fluid
            search
            selection
            options={this.props.servers}
        />)
    }
}