import React, { Component } from "react"
import ServerSearch from "./ServerSearch"
import PlayerSearch from "./PlayerSearch"
import PlayerIndex from "../../utilities/PlayerIndex"

export default class SearchBar extends Component {
  constructor(props) {
    super(props)

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

  handleServerChange(e, { value }) {
    this.setState({ 
      selectedServer: value,
      playerListIsLoading: true
     })

    new PlayerIndex().GetIndex(value).then((indexItems) => {
      this.setState({
        playerIndex: indexItems.map((item) => {
          return {
            key: item.LodestoneId,
            value: item.LodestoneId,
            text: item.PlayerName,
            lowercasetext: item.PlayerName.toLowerCase()
          }
        }),
        playerListIsLoading: false
      })
    })
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