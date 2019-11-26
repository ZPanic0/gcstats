import React, { Component } from "react"
import { Dropdown } from "semantic-ui-react"

export default class PlayerSearch extends Component {
    render() {
        return (
            <Dropdown
                selectOnNavigation={false}
                onSearchChange={this.props.handleSearchChange}
                onChange={this.props.handleChange}
                options={this.props.players}
                search
                selection
                value={this.props.value}
                noResultsMessage={"Enter a character name."}
                loading={this.props.loading}
                disabled={this.props.disabled}
            />
        )
    }
}