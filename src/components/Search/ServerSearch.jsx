import React from "react"
import { Dropdown } from "semantic-ui-react"

//TODO: make global servers list since it keeps getting reused
const servers = [
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
].map((server) => { return { key: server, value: server, text: server } })

export default class ServerSearch extends React.Component {
    render() {
        return (
            <Dropdown
                onChange={this.props.handleChange}
                options={servers}
                placeholder="Select Server..."
                selection
                search
                value={this.props.value}
            />
        )
    }
}