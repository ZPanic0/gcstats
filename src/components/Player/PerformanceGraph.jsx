import React, { Component } from "react"
import { ResponsiveContainer, LineChart, Line, XAxis, YAxis } from "recharts"
import { Checkbox, Menu } from "semantic-ui-react"

export default class PerformanceGraph extends Component {
    render() {
        return (
            <>
                <ResponsiveContainer width="100%" height={200}>
                    <LineChart data={this.props.Performances}>
                        <Line type="monotone" dataKey="Score" stroke="#8884d8" />
                        <XAxis dataKey="WeekStart" />
                        <YAxis />
                    </LineChart>
                </ResponsiveContainer>
                <Menu floated="right">
                    <Menu.Item>Show empty weeks</Menu.Item>
                    <Menu.Item>
                        <Checkbox slider onChange={this.props.onShowWeeksChange} checked={this.props.checked}/>
                    </Menu.Item>
                </Menu>
            </>
        )
    }
}