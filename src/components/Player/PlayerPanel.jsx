import React, { Component } from "react"
import { Grid } from "semantic-ui-react"
import PlayerCard from "./PlayerCard"
import PerformanceGraph from "./PerformanceGraph"
import SummaryTable from "./SummaryTable"
import LodestoneCalendar from "../../utilities/LodestoneCalendar"

export default class PlayerPanel extends Component {
    constructor(props) {
        super(props)

        this.handleShowEmptyWeeksToggleChange = this.handleShowEmptyWeeksToggleChange.bind(this)
    }

    state = {
        showEmptyWeeks: false
    }

    handleShowEmptyWeeksToggleChange(e, { checked }) {
        this.setState({ showEmptyWeeks: checked })
    }

    render() {
        const calendar = new LodestoneCalendar(this.props.player.Performances)

        return (
            <Grid divided="vertically" key={this.props.player.LodestoneId}>
                <Grid.Row>
                    <Grid.Column width={16} />
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column width={6}>
                        <PlayerCard

                            PlayerName={this.props.player.PlayerName}
                            PortraitUrl={this.props.player.PortraitUrl}
                            LodestoneId={this.props.player.LodestoneId}
                            Faction={this.props.player.Faction}
                            FactionRank={this.props.player.FactionRank}
                            Server={this.props.player.Server}
                        />
                    </Grid.Column>
                    <Grid.Column width={10}>
                        <PerformanceGraph
                            Performances={calendar.GetMappedSet(this.state.showEmptyWeeks)}
                            onShowWeeksChange={this.handleShowEmptyWeeksToggleChange}
                            checked={this.state.showEmptyWeeks} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column width={16}>
                        <SummaryTable Performances={calendar.GetMappedSet(false)} />
                    </Grid.Column>
                </Grid.Row>
            </Grid>)
    }
}