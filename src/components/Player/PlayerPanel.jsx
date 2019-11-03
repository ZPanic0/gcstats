import React, { Component } from "react"
import moment from "moment"
import { Grid } from "semantic-ui-react"
import PlayerCard from "./PlayerCard"
import PerformanceGraph from "./PerformanceGraph"
import SummaryTable from "./SummaryTable"

export default class PlayerPanel extends Component {
    simulateLodestoneCalendar(callback) {
        const now = new Date()
        const startDate = moment("2014-01-06T00:00:00")
        const endDate = moment(now).add(-7 - (7 + (now.getDay() - 1)) % 7, 'd')
        const formatString = "M/D/Y"

        for (let currentYear = 2014, currentWeek = 0; startDate.isBefore(endDate); startDate.add(7, 'd')) {
            const startDateYear = startDate.year()

            if (startDateYear === currentYear) {
                currentWeek++
            } else {
                currentWeek = 1
                currentYear = startDateYear
            }

            const tallyingPeriodId = currentYear * 100 + currentWeek
            callback(tallyingPeriodId, startDate.format(formatString), moment(startDate).add(6, "d").format(formatString))
        }
    }


    mapPerformanceToGraphSet(reportEmptyWeeks) {
        let performanceIterator = this.props.player.Performances.entries()
        let thisPerformanceIteration = performanceIterator.next()
        thisPerformanceIteration.TallyingPeriodId = thisPerformanceIteration.value[1].IndexId / 100000 << 0

        const mappedSet = new Map()

        this.simulateLodestoneCalendar((tallyingPeriodId, weekStart, weekEnd) => {
            mappedSet.set(tallyingPeriodId, {
                TallyingPeriodId: tallyingPeriodId,
                Score: 0,
                WeekStart: weekStart,
                WeekEnd: weekEnd
            })

            if (!thisPerformanceIteration.done) {
                while (thisPerformanceIteration.TallyingPeriodId === tallyingPeriodId) {
                    let thisPerformance = thisPerformanceIteration.value[1]
                    mappedSet.get(thisPerformanceIteration.TallyingPeriodId).Score += thisPerformance.Score

                    thisPerformanceIteration = performanceIterator.next()
                    thisPerformanceIteration.TallyingPeriodId = thisPerformance.IndexId / 100000 << 0
                }
            }
        })

        const result = Array.from(mappedSet.values())
        return reportEmptyWeeks ? result : result.filter((item) => item.Score > 0)
    }

    mapPerformanceToSummaryTableSet() {
        const set = []
        let performanceIterator = this.props.player.Performances.entries()
        let thisPerformanceIteration = performanceIterator.next()
        thisPerformanceIteration.TallyingPeriodId = thisPerformanceIteration.value[1].IndexId / 100000 << 0

        this.simulateLodestoneCalendar((tallyingPeriodId, weekStart, weekEnd) => {
            if (!thisPerformanceIteration.done) {
                while (thisPerformanceIteration.TallyingPeriodId === tallyingPeriodId) {
                    let thisPerformance = thisPerformanceIteration.value[1]

                    set.push({
                        WeekStart: weekStart,
                        WeekEnd: weekEnd,
                        IndexId: thisPerformance.IndexId,
                        Score: thisPerformance.Score,
                        Rank: thisPerformance.Rank,
                        Faction: thisPerformance.Faction
                    })

                    thisPerformanceIteration = performanceIterator.next()
                    thisPerformanceIteration.TallyingPeriodId = thisPerformance.IndexId / 100000 << 0
                }
            }
        })

        return set
    }

    render() {
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
                        <PerformanceGraph Performances={this.mapPerformanceToGraphSet(false)} />
                    </Grid.Column>
                </Grid.Row>
                <Grid.Row>
                    <Grid.Column width={16}>
                        <SummaryTable Performances={this.mapPerformanceToSummaryTableSet()} />
                    </Grid.Column>
                </Grid.Row>
            </Grid>)
    }
}