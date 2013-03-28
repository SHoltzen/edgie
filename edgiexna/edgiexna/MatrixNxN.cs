using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edgiexna
{
    class dMatrixNxN
    {
        public dMatrixNxN(int N)
        {
            n = N;
            matrix = new double[N * N];
        }

        private int n;
        private double[] matrix;
    }
}
