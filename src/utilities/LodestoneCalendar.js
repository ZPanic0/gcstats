import moment from "moment"

export default class LodestoneCalendar {
    constructor(performances) {
        this.setWithEmpties = new Map()
        this.setWithoutEmpties = new Map()

        const formatString = "M/D/Y"
        const iterator = performances.entries()
        let iteration = iterator.next()

        for (let now = new Date(),
            currentYear = 2014,
            currentWeek = 0,
            currentDate = moment("2014-01-06T00:00:00"),
            endDate = moment(now).add(-7 - (7 + (now.getDay() - 1)) % 7, "d"); currentDate.isBefore(endDate); currentDate.add(7, "d")) {
            const year = currentDate.year()

            if (currentYear === year) {
                currentWeek++
            } else {
                currentWeek = 1
                currentYear = year
            }

            const currentTallyingPeriodId = currentYear * 100 + currentWeek

            this.setWithEmpties.set(currentTallyingPeriodId, {
                TallyingPeriodId: currentTallyingPeriodId,
                Score: 0,
                WeekStart: currentDate.format(formatString),
                WeekEnd: moment(currentDate).add(6, "d").format(formatString)
            })

            while (!iteration.done && currentTallyingPeriodId === iteration.value[1].IndexId / 100000 << 0) {
                let currentPerformance =  iteration.value[1]

                this.setWithEmpties.get(currentTallyingPeriodId).Score += currentPerformance.Score

                if (this.setWithoutEmpties.has(currentTallyingPeriodId)) {
                    this.setWithoutEmpties.get(currentTallyingPeriodId).Score += currentPerformance.Score
                } else {
                    this.setWithoutEmpties.set(currentTallyingPeriodId, {
                        WeekStart: currentDate.format(formatString),
                        WeekEnd: moment(currentDate).add(6, "d").format(formatString),
                        Score: currentPerformance.Score,
                        Faction: currentPerformance.Faction,
                        Rank: currentPerformance.Rank,
                        IndexId: currentPerformance.IndexId
                    })
                }

                iteration = iterator.next()
            }
        }
    }

    GetMappedSet(withEmptyWeeks) {
        return Array.from((withEmptyWeeks ? this.setWithEmpties : this.setWithoutEmpties).values())
    }
}