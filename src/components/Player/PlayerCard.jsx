import React, { Component } from "react"
import { Card, Image, Icon, Popup } from 'semantic-ui-react'
import Servers from "../../utilities/Servers"

export default class PlayerCard extends Component {
    getDataCenter(serverId) {
        if (isNaN(serverId) || serverId < 1 || serverId > 68) {
            throw new Error("Invalid Server Id")
        }
        if (serverId <= 10) {
            return "Elemental"
        }
        if (serverId <= 21) {
            return "Gaia"
        }
        if (serverId <= 32) {
            return "Mana"
        }
        if (serverId <= 40) {
            return "Aether"
        }
        if (serverId <= 48) {
            return "Primal"
        }
        if (serverId <= 56) {
            return "Crystal"
        }
        if (serverId <= 62) {
            return "Chaos"
        }
        return "Light"
    }

    getFactionStyles() {
        switch (this.props.Faction) {
            case 1:
                return "maelstrom-gradient"
            case 2:
                return "adder-gradient"
            case 3:
                return "flames-gradient"
            default:
                return ""
        }
    }

    onClipboardCopyClick() {
        window.navigator.clipboard.writeText(`${window.location.href}`)
    }

    render() {
        const avatarUrl = this.props.PortraitUrl
            ? `https://img2.finalfantasyxiv.com/f/${this.props.PortraitUrl}_96x96.jpg`
            : "https://img.finalfantasyxiv.com/lds/h/z/6PLTZ82M99GJ7tKOee1RSwvNrQ.png"

        return (
            <Card className={this.getFactionStyles()}>
                <Card.Content>
                    <Image src={avatarUrl} size="tiny" floated="left" rounded />
                    <Card.Header>
                        <a
                            title="Go to player lodestone page"
                            href={`https://na.finalfantasyxiv.com/lodestone/character/${this.props.LodestoneId}/`}>
                            {this.props.PlayerName}
                        </a>
                    </Card.Header>
                    <Card.Meta>
                        <span>{Servers[this.props.Server]} ({this.getDataCenter(this.props.Server)})</span>
                    </Card.Meta>
                    <Popup
                        content="Copied!"
                        on="click"
                        trigger={<Icon
                            style={{ paddingTop: "2px" }}
                            link
                            name="linkify"
                            title="Copy page to clipboard"
                            onClick={this.onClipboardCopyClick}
                        />}
                    />
                </Card.Content>
                <Card.Content extra style={{ paddingTop: "0", paddingBottom: "0" }}>
                    <Image
                        src={`../gc/${this.props.FactionRank - 1}.png`}
                        size="mini"
                        floated="left"
                    />
                </Card.Content>
            </Card>
        )
    }
}