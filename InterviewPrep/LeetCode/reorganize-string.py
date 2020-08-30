class Solution:
    def reorganizeString(self, S: str) -> str:
        charDict = {}
        for c in S:
            if c in charDict:
                charDict[c] = charDict[c] + 1
            else:
                charDict[c] = 1
        maxVal = max(charDict.values())
        
        result = ''
        if maxVal > ((len(S) / 2) + 0.5):
            return result
        
        while len(result) != len(S):
            sortedKeys = sorted(charDict, key=charDict.get, reverse=True)
            result = result + sortedKeys[0]
            charDict[sortedKeys[0]] = charDict[sortedKeys[0]] - 1
            if charDict[sortedKeys[1]] > 0:
                result = result + sortedKeys[1]
                charDict[sortedKeys[1]] = charDict[sortedKeys[1]] - 1

        return result

sol = Solution()
result = sol.reorganizeString("aaab")
print(result)
result = sol.reorganizeString("aab")
print(result)
result = sol.reorganizeString("aaacb")
print(result)