using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Workflow
{
    public static class Timer
    {
        public static async Task Execute(Action action, TimeSpan delay)
        {
            
            await Task.Delay(delay, new System.Threading.CancellationToken());
            await Task.Run(action);
        }
    }
}
