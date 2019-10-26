import React, { Component } from "react"
import { Menu, Icon } from "semantic-ui-react"
import Clamp from "../../utilities/Clamp"

export default class PageMenu extends Component {
    constructor(props) {
        super(props)

        this.handleOnClick = this.handleOnClick.bind(this)
    }

    getPageMenuItems() {
        const items = [
            <Menu.Item
                key="leftChevron"
                disabled={this.props.selectedPage < this.props.pages - 2 || this.props.pages === 1}
                as='a'
                icon
                value={-5}
                onClick={this.handleOnClick}
            >
                <Icon name='chevron left' />
            </Menu.Item>
        ]

        for (let buttonValue = -2; buttonValue < 3; buttonValue++) {
            const pageValue = this.props.selectedPage + buttonValue

            if (pageValue > 0 && pageValue <= this.props.pages) {
                items.push(
                    <Menu.Item
                        key={buttonValue}
                        as="a"
                        value={buttonValue}
                        active={buttonValue === 0}
                        onClick={this.handleOnClick}
                    >
                        {this.props.selectedPage + buttonValue}
                    </Menu.Item>
                )
            }
        }

        items.push(
            <Menu.Item
                key="rightChevron"
                disabled={this.props.selectedPage > 3 || this.props.pages === 1}
                as='a'
                icon
                value={5}
                onClick={this.handleOnClick}
            >
                <Icon name='chevron right' />
            </Menu.Item>
        )

        return items
    }

    handleOnClick(e, { value }) {
        this.props.onSelect(Clamp(this.props.selectedPage + value, 1, this.props.pages))
    }

    render() {
        return (
            <Menu floated={this.props.floated} pagination>
                {this.getPageMenuItems()}
            </Menu>
        )
    }
}