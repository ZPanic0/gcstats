import React, { Component } from "react"
import ServerSearch from "./ServerSearch"
import PlayerSearch from "./PlayerSearch"

export default class SearchBar extends Component {
  constructor(props) {
    super(props)

    this.fetchPlayerIndex = this.fetchPlayerIndex.bind(this)
    this.handleServerChange = this.handleServerChange.bind(this)
    this.handlePlayerSearchTextChange = this.handlePlayerSearchTextChange.bind(this)
    this.handlePlayerSelectionChange = this.handlePlayerSelectionChange.bind(this)
  }

  state = {
    playerIndex: [],
    playerSearchResults: [],
    selectedServer: "",
    selectedLodestoneId: "",
    selectedPlayerName: "",
    playerListIsLoading: false
  }

  fetchPlayerIndex(server) {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", `/indexes/${server}.bin`, true);
    xhr.responseType = "arraybuffer";
    xhr.onload = () => {
      var message = this.props.IndexMessage.decode(new Uint8Array(xhr.response))

      var items = message.IndexEntries.map(item => {
        return {
          key: item.LodestoneId,
          value: item.LodestoneId,
          text: item.PlayerName,
          lowercasetext: item.PlayerName.toLowerCase()
        }
      })

      this.setState({
        playerIndex: items,
        playerListIsLoading: false
      })
    }
    this.setState({ playerListIsLoading: true })
    xhr.send(null);
  }

  handleServerChange(e, { value }) {
    this.fetchPlayerIndex(value)
    this.setState({ selectedServer: value })
  }

  handlePlayerSearchTextChange(event, data) {
    const loweredQuery = data.searchQuery.toLowerCase()
    this.setState({
      playerSearchResults: data.searchQuery.length < 3
        ? []
        : this.state.playerIndex.filter(playerName => playerName.lowercasetext.includes(loweredQuery))
    })
  }

  handlePlayerSelectionChange(event, { value }) {
    this.setState({ selectedLodestoneId: value })
    this.props.handleSearchSelection(value)
  }

  render() {
    return (
      <div>
        <ServerSearch
          handleChange={this.handleServerChange}
          value={this.state.selectedServer}
        />
        <PlayerSearch
          handleChange={this.handlePlayerSelectionChange}
          handleSearchChange={this.handlePlayerSearchTextChange}
          players={this.state.playerSearchResults}
          value={this.state.selectedLodestoneId}
          isLoading={this.state.playerListIsLoading}
        />
      </div>
    )
  }
}