import React, { Component } from "react"
import { ResponsiveContainer, LineChart, Line, XAxis, YAxis } from "recharts"

export default class PerformanceGraph extends Component {
    render() {
        return (
            <ResponsiveContainer width="100%" height={200}>
                <LineChart data={this.props.Performances}>
                    <Line type="monotone" dataKey="Score" stroke="#8884d8" />
                    <XAxis dataKey="WeekStart" />
                    <YAxis />
                </LineChart>
            </ResponsiveContainer>
        )
    }
}