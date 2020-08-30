from typing import List
class Solution:
    def findJudge(self, N: int, trust: List[List[int]]) -> int:
        trustCount = {}
        for i in range(N):
            trustCount[i+1] = {"trusts": 0, "trusted": 0}
        for entry in trust:
            trustCount[entry[0]]["trusts"] += 1
            trustCount[entry[1]]["trusted"] += 1
        couldBeJudge = None
        for i in range(N):
            entry = trustCount[i + 1]
            if entry["trusts"] == 0 and entry["trusted"] == N -1:
                if couldBeJudge is not None:
                    return -1
                couldBeJudge = i + 1
        if couldBeJudge is None:
            return -1
        return couldBeJudge

sol = Solution()
result = sol.findJudge(2, [[1,2]])
print(result)