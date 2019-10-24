import React, { Component } from "react"
import { Card, Image } from 'semantic-ui-react'

const servers = [
    "NoInput",
    "Aegis",
    "Atomos",
    "Carbuncle",
    "Garuda",
    "Gungnir",
    "Kujata",
    "Ramuh",
    "Tonberry",
    "Typhon",
    "Unicorn",
    "Alexander",
    "Bahamut",
    "Durandal",
    "Fenrir",
    "Ifrit",
    "Ridill",
    "Tiamat",
    "Ultima",
    "Valefor",
    "Yojimbo",
    "Zeromus",
    "Anima",
    "Asura",
    "Belias",
    "Chocobo",
    "Hades",
    "Ixion",
    "Mandragora",
    "Masamune",
    "Pandaemonium",
    "Shinryu",
    "Titan",
    "Adamantoise",
    "Cactuar",
    "Faerie",
    "Gilgamesh",
    "Jenova",
    "Midgardsormr",
    "Sargatanas",
    "Siren",
    "Behemoth",
    "Excalibur",
    "Exodus",
    "Famfrit",
    "Hyperion",
    "Lamia",
    "Leviathan",
    "Ultros",
    "Balmung",
    "Brynhildr",
    "Coeurl",
    "Diabolos",
    "Goblin",
    "Malboro",
    "Mateus",
    "Zalera",
    "Cerberus",
    "Louisoix",
    "Moogle",
    "Omega",
    "Ragnarok",
    "Spriggan",
    "Lich",
    "Odin",
    "Phoenix",
    "Shiva",
    "Twintania",
    "Zodiark"
]

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
    //TODO: switch this to xivapi's numbering system or rebuild the numbers to match what is already in use when building sprite sheet
    convertFactionRankToXivapiIndex(factionRank) {
        switch (factionRank) {
            case 2:
            case 21:
            case 40:
                return 1
            case 3:
            case 22:
            case 41:
                return 2
            case 4:
            case 23:
            case 42:
                return 3
            case 5:
            case 24:
            case 43:
                return 4
            case 6:
            case 25:
            case 44:
                return 5
            case 7:
            case 26:
            case 45:
                return 6
            case 8:
            case 27:
            case 46:
                return 7
            case 9:
            case 28:
            case 47:
                return 8
            case 10:
            case 29:
            case 48:
                return 9
            case 11:
            case 30:
            case 49:
                return 10
            case 12:
            case 31:
            case 50:
                return 11
            case 13:
            case 32:
            case 51:
                return 12
            case 14:
            case 33:
            case 52:
                return 13
            case 15:
            case 34:
            case 53:
                return 14
            case 16:
            case 35:
            case 54:
                return 15
            case 17:
            case 36:
            case 55:
                return 16
            case 18:
            case 37:
            case 56:
                return 17
            case 19:
            case 38:
            case 57:
                return 18
            case 20:
            case 39:
            case 58:
                return 19
            case 0:
            case 1:
            default:
                throw new Error("Invalid FactionRank")
        }
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
    render() {
        return (
            <Card className={this.getFactionStyles()}>
                <Card.Content>
                    <Image src={`https://img2.finalfantasyxiv.com/${this.props.PortraitUrl}`} size="tiny" floated="left" />
                    <Card.Header>
                        <a href={`https://na.finalfantasyxiv.com/lodestone/character/${this.props.LodestoneId}/`}>{this.props.PlayerName}</a>
                    </Card.Header>
                    <Card.Meta>
                        <span>{servers[this.props.Server]} ({this.getDataCenter(this.props.Server)})</span>
                    </Card.Meta>
                    <Image
                        src={`https://xivapi.com/img-misc/gc/character_gc_${this.props.Faction}_${this.convertFactionRankToXivapiIndex(this.props.FactionRank)}.png`}
                        size="mini"
                        floated="left"
                    />
                </Card.Content>
            </Card>
        )
    }
}