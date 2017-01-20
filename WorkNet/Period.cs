using System;
using System.Collections.Generic;
using System.Text;

namespace WorkNet
{
    public class Period
    {
        public int pre_year, pre_month, pre_lastDay;
        public int year, month, lastDay;
        public int min_salaryday;
        public int extraday;
        public int[] days;
        Calendar calendar;
        Calendar pre_calendar;

        public DateTime eDate, sDate;

        public Period()
        {
        }

        public int Length
        {
            get { return days.Length; }
        }

        public int this[int i]
        {
            get { return days[i]; }
            set { days[i] = value; }
        }

        public bool Load(int year, int month, int min_salaryday)
        {
            this.min_salaryday = min_salaryday;
            this.year = year;
            this.month = month;

            if (month == 1)
            {
                pre_month = 12;
                pre_year = year - 1;
            }
            else
            {
                pre_year = year;
                pre_month = month - 1;
            }
            int i;

            if (min_salaryday > 1)
            {
                extraday = 32 - min_salaryday;
                pre_calendar = new Calendar(DB.connection);
                pre_calendar.Load(pre_year, pre_month);
                pre_lastDay = pre_calendar.lastDay;
            }
            else extraday = 0;

            days = new int[31 + extraday];
            calendar = new Calendar(DB.connection);
            bool B = calendar.Load(year, month);

            lastDay = calendar.lastDay;

            for (i = 0; i < extraday; i++)
                days[i] = pre_calendar.days[i + min_salaryday - 1];

            for (i = 0; i < 31; i++)
                days[i + extraday] = calendar.days[i];

            eDate = new DateTime(calendar.year, calendar.month, calendar.lastDay);
            sDate = new DateTime(calendar.year, calendar.month, 1);

            return B;
        }
    }
}
