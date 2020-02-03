using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP.Models
{
    public class Checkpoint
    {
        public int SnaphotId { get; private set; }

        public Checkpoint(int id) 
        {
            SnaphotId = id;
        }
    }
}
